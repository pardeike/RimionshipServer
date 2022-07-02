sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout server.key -out server.crt -config generate.conf
sudo openssl pkcs12 -export -out server.pfx -inkey server.key -in server.crt

cp server.pfx ../RimionshipServer
