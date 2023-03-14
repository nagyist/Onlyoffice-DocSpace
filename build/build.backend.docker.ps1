Param (
  [switch]$https,
  [switch]$no_ds
)

$PSversionMajor = $PSVersionTable.PSVersion | sort-object major | ForEach-Object { $_.major }
$PSversionMinor = $PSVersionTable.PSVersion | sort-object minor | ForEach-Object { $_.minor }

if ($PSversionMajor -lt 7 -or $PSversionMinor -lt 2) {
  Write-Error "Powershell version must be greater than or equal to 7.2."
  exit
}

$Branch = git branch --show-current
$BranchExistRemote = git ls-remote --heads origin $Branch

if (-not $BranchExistRemote) {
  Write-Error "The current branch does not exist in the remote repository. Please push changes."
  exit
} 

$RootDir = Split-Path -Parent $PSScriptRoot
$DockerDir = ($RootDir + "\build\install\docker")
$BuildDate = Get-Date -Format "yyyy-MM-dd"
$LocalIp = (Get-CimInstance -ClassName Win32_NetworkAdapterConfiguration | Where-Object { $_.DHCPEnabled -ne $null -and $_.DefaultIPGateway -ne $null }).IPAddress | Select-Object -First 1

$Doceditor = ($LocalIp + ":5013")
$Login = ($LocalIp + ":5011")
$Client = ($LocalIp + ":5001")

$DockerFile = "Dockerfile.dev"
$EnvExtension = "dev"
$CoreBaseDomain = "localhost"

# Stop all backend services"
& "$PSScriptRoot\start\stop.backend.docker.ps1"

$Env:COMPOSE_IGNORE_ORPHANS = "True"

$Containers = docker ps -a -f "name=^onlyoffice" --format="{{.ID}} {{.Names}}" | Select-String -Pattern ("mysql|rabbitmq|redis|elasticsearch|documentserver") -NotMatch | ConvertFrom-String | ForEach-Object P1
$Images = docker images onlyoffice/docspace* -q

if ($Containers) {
  Write-Host "Remove all backend containers" -ForegroundColor Blue
  docker rm -f $Containers
}

if ($Images) {
  Write-Host "Remove all docker images except 'mysql, rabbitmq, redis, elasticsearch, documentserver'" -ForegroundColor Blue
  docker rmi -f $Images
}

Write-Host "Run MySQL" -ForegroundColor Green
docker compose -f  ($DockerDir + "\db.yml") up -d

Write-Host "Run environments (redis, rabbitmq)" -ForegroundColor Green
$Env:DOCKERFILE = $DockerFile
docker compose -f ($DockerDir + "\redis.yml") -f ($DockerDir + "\rabbitmq.yml") up -d

if ($no_ds) {
  Write-Host "SKIP Document server" -ForegroundColor Blue
} else { 
  Write-Host "Run Document server" -ForegroundColor Green
  $Env:DOCUMENT_SERVER_IMAGE_NAME = "onlyoffice/documentserver-de:latest"
  $Env:ROOT_DIR = $RootDir
  docker compose -f ($DockerDir + "\ds.dev.yml") up -d
}

Write-Host "Build all backend services" -ForegroundColor Blue
$Env:DOCKERFILE = $DockerFile
$Env:RELEASE_DATE = $BuildDate
$Env:GIT_BRANCH = $Branch
$Env:SERVICE_DOCEDITOR = $Doceditor
$Env:SERVICE_LOGIN = $Login
$Env:SERVICE_CLIENT = $Client
$Env:APP_CORE_BASE_DOMAIN = $CoreBaseDomain
$Env:ENV_EXTENSION = $EnvExtension
docker compose -f ($DockerDir + "\build.dev.yml") build --build-arg GIT_BRANCH=$Branch --build-arg RELEASE_DATE=$BuildDate

Write-Host "Run DB migration" -ForegroundColor Green
$Env:DOCKERFILE = $DockerFile
docker compose -f ($DockerDir + "\migration-runner.yml") up -d

# Start all backend services"
& "$PSScriptRoot\start\start.backend.docker.ps1"

if ($https) {
  $ConfigPath = ($RootDir + "/config/nginx/includes/onlyoffice-ssl.conf")
  $ContainerRunning = docker inspect --format '{{json .State.Running}}' onlyoffice-proxy

  if ($ContainerRunning -eq $True) {
    if (Test-Path -Path $ConfigPath -PathType Leaf) {
      docker cp $ConfigPath onlyoffice-proxy:etc/nginx/includes/onlyoffice-ssl.conf
      Write-Host "HTTPS settings applied" -ForegroundColor Green
      docker restart onlyoffice-proxy
      Write-Host "Contatiner restarted" -ForegroundColor Blue
    } else {
      Write-Error "Config file 'onlyoffice-ssl.conf' not exist"
      exit
    }
  } else {
    Write-Error "Proxy container is not running"
    exit
  }
}