sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -config localhost.conf -passin pass:foo123
sudo openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt

cp localhost.pfx ..
