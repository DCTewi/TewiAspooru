# TewiBoard

[中文版本点击这里](./README-zh_CN.md)

A imageboard template by ASP.NET Core.

> **Some Warning Before Use:**
>
> This imageboard template is made just for fun. There are many code that isn't safe enough. If you wanna use this frame to deploy an imageboard for a formal or a longrange use, please upgrade the algorithm of the UserID generator and the delete, report methods. 

## Previews

![](F:\_Projects\_Web\TewiBoard\TewiBoard.Previews\HomePage.png)

![](F:\_Projects\_Web\TewiBoard\TewiBoard.Previews\Timeline.png)

![](F:\_Projects\_Web\TewiBoard\TewiBoard.Previews\Search.png)

![](F:\_Projects\_Web\TewiBoard\TewiBoard.Previews\Post.png)

![](F:\_Projects\_Web\TewiBoard\TewiBoard.Previews\Reply.png)

## Features

- Easy to deploy
- Easy to localize
- Based on ASP.Net Core 2.1 or higher
- Based on MySQL

## Building

> This is a template building procedure on Ubuntu 16.04 LTS.
>
>  You need put your .crt and .key for SSL.

1. **Create a board-admin user**

   ```bash
   addusr tewiboard # Create user
   usermod -aG sudo tewiboard # Add user to sudo group
   su tewiboard # Switch to new user
   ```

2. **Install .Net Core**

   ```bash
   cd ~ # cd the home path
   sudo wget https://dot.net/v1/dotnet-install.sh
   sudo bash ./dotnet-install.sh -c Current
   export PATH=$PATH:/home/tewiboard/.dotnet
   source /etc/profile
   ```

   Go next step if you can type `dotnet --version` correctly.

3. **Install MySQL**

   ```bash
   sudo apt install mysql-server
   sudo apt install mysql-client
   sudo apt install libmysqlclient-dev
   ```

4. **Create database for tewiboard**

   Get MySQL console by:

   ```bash
   mysql -u root -p
   ```

   , and type your password. Then you can get MySQL console, and for next you can create database and table like this:

   ```mysql
   create database tewiboard_sql;
   use `tewiboard_sql`;
   drop table if exists `card`;
   create table `card` (
   	`pid` bigint(12) not null auto_increment,
       `genre` varchar(30) default 'Complex',
       `module` varchar(30) default 'ComplexTotal',
       `replyid` bigint(12) default null,
       `replytop` bigint(12) default null,
       `title` varchar(30) default 'Untitled',
       `usernick` varchar(30) default 'Anonymous',
       `userid` tinytext default null,
       `isred` int default 0,
       `posttime` datetime default now(),
       `content` text not null,
       `imgurl` text default null,
       primary key (`pid`)
   )engine=InnoDB default charset=utf8mb4;
   exit;
   ```
   
5. **Install nginx**

   ```bash
   sudo apt install nginx
   ```
   
6. **Custom your board**

   The files you should customize:

   ```bash
   # ./Controllers/CardController.cs
   	`deletepwd` for delete cards.
   # ./Models/Genres.cs
   	Change your Genres and Modules name and add some RedName Key
   # ./Views/*.cshtml
   	You can change the view text by editing the variables on the top of .cshtml files.
   # appsettings.json
   	The MySQL server name and password
   ```
   
7. **Publish your board**
   
   Copy the folder` /TewiBoard` to `~/TewiBoard`
   
   ```bash
   cd ~/TewiBoard
   dotnet publish -c Release
   scp -r ~/TewiBoard/bin/release/netcoreapp2.1/publish/* /var/www/tewiboard
   # Or ~/TewiBoard/bin/release/netcoreapp2.2/publish/*
   ```
   
8. **Configure your board service**

   Add a service file:

   ```bash
   sudo vim /etc/systemd/system/tewiboard.service
   ```

   And add this:

   ```bash
   [Unit]
       Description=tewiboard
       [Service]
       WorkingDirectory=/var/www/tewiboard
       ExecStart=/home/TewiBoard/.dotnet/dotnet /var/www/tewiboard/TewiBoard.Web.dll
       Restart=always
       RestartSec=10    # Restart service after 10 seconds if dotnet service crashes
       SyslogIdentifier=dotnet-tewiboard
       User=www-data
       Environment=ASPNETCORE_ENVIRONMENT=Production
   
       [Install]
       WantedBy=multi-user.target
   ```

   Then start the service:

   ```bash
   sudo systemctl enable tewiboard.service
   sudo systemctl start tewiboard.service
   sudo systemctl status tewiboard.service
   ```

   If it's actived, go next.

9. **Configure your nginx**

   and set your `/etc/nginx/site-available/default` as:
   
   ```nginx
   server {
       listen 80;
       server_name YOUR.DOMAIN.COM;
       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
           proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
           proxy_set_header   X-Forwarded-Proto $scheme;
       }
   }
   ```
   
   , set your `/etc/nginx/proxy.conf` as:
   
   ```nginx
   proxy_redirect          off;
   proxy_set_header        Host $host;
   proxy_set_header        X-Real-IP $remote_addr;
   proxy_set_header        X-Forwarded-For $proxy_add_x_forwarded_for;
   proxy_set_header        X-Forwarded-Proto $scheme;
   client_max_body_size    10m;
   client_body_buffer_size 128k;
   proxy_connect_timeout   90;
   proxy_send_timeout      90;
   proxy_read_timeout      90;
   proxy_buffers           32 4k;
   ```
   
   , set your `/etc/nginx/nginx.conf` as:
   
   ```nginx
   user root;
   
   worker_processes 1;
   error_log /usr/local/nginx/logs/error.log info;
   worker_rlimit_nofile 65535;
   
   events
   {
       use epoll;
       worker_connections 65535;
   }
   
   http {
       include        /etc/nginx/proxy.conf;
       limit_req_zone $binary_remote_addr zone=one:10m rate=5r/s;
       server_tokens  off;
   
       sendfile on;
       keepalive_timeout   29; # Adjust to the lowest possible value that makes sense for your use case.
       client_body_timeout 10; client_header_timeout 10; send_timeout 10;
   
       upstream tewiboard{
           server localhost:5000;
       }
   
       server {
           listen     *:80;
           add_header Strict-Transport-Security max-age=15768000;
           return     301 https://$host$request_uri;
       }
   
       server {
           listen                    *:443 ssl;
           server_name               YOUR.DOMAIN.COM;
           ssl_certificate           YOUR_SSL_CRT.crt;
           ssl_certificate_key       YOUR_SSL_KEY.key;
           ssl_protocols             TLSv1.1 TLSv1.2;
           ssl_prefer_server_ciphers on;
           ssl_ciphers               "EECDH+AESGCM:EDH+AESGCM:AES256+EECDH:AES256+EDH";
           ssl_ecdh_curve            secp384r1;
           ssl_session_cache         shared:SSL:10m;
           ssl_session_tickets       off;
           ssl_stapling              on; #ensure your cert is capable
           ssl_stapling_verify       on; #ensure your cert is capable
   
           add_header Strict-Transport-Security "max-age=63072000; includeSubdomains; preload";
   		add_header X-Frame-Options DENY;
   		add_header X-Content-Type-Options "nosniff";
   
           #Redirects all traffic
           location / {
               proxy_pass http://tewiboard;
               limit_req  zone=one burst=10 nodelay;
           }
       }
   }
   ```
   
   Then reload your nginx by:
   
   ```bash
   sudo nginx -s reload
   ```

Then you can access your board site now!

## Contact

- The author - DCTewi (dctewi@dctewi.com)

## Lincense

![License-MIT](https://img.shields.io/badge/license-MIT-66ccff.svg)