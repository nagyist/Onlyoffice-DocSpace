@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\ASC.Identity\bin\Debug\net6.0\ASC.Identity.exe urls=http://0.0.0.0:5033 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=identity pathToConf=%cd%\config