ssh daidakaram.com "rm -rf /var/www/daidakaram.com/server/*"
scp -r bin/ daidakaram.com:/var/www/daidakaram.com/server
scp Global.asax daidakaram.com:/var/www/daidakaram.com/server
scp web.config daidakaram.com:/var/www/daidakaram.com/server
scp ../daidakaram.sql daidakaram.com:/var/www/daidakaram.com/server
ssh daidakaram.com "sh /root/daidakaram-configure-db.sh"
