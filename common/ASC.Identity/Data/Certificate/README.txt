https://gist.github.com/marta-krzyk-dev/83168c9a8e985e5b3b1b14a98b533b9c

generate ECSDA private key

openssl ecparam -genkey -name secp521r1 -noout -out private-key.pem

https://wiki.openssl.org/index.php/Command_Line_Elliptic_Curve_Operations

encrypted private key pkcs8

openssl pkcs8 -topk8 -in private-key.pem -out encrypted-private-key.pem