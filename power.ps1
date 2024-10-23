param($t,$u,$p)

#docker build 
Write-Host "Hello $t"

docker -H 192.168.51.153:2375 login --username=$u registry.cn-hangzhou.aliyuncs.com --password=$p

$ServiceDockerfilePath="./src/Services/Masa.Auth.Service.Admin/Dockerfile"
$ServiceServerName="masa-auth-service-admin"
$WebDockerfilePath="./src/Web/Masa.Auth.Web.Admin.Server/Dockerfile"
$WebServerName="masa-auth-web-admin"
$SsoDockerfilePath="./src/Web/Masa.Auth.Web.Sso/Dockerfile"
$SsoServerName="masa-auth-web-sso"

docker -H 192.168.51.153:2375 build -t registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t  -f $ServiceDockerfilePath .
docker -H 192.168.51.153:2375 push registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t 

docker -H 192.168.51.153:2375 build -t registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t  -f $WebDockerfilePath .
docker -H 192.168.51.153:2375 push registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t 

docker -H 192.168.51.153:2375 build -t registry.cn-hangzhou.aliyuncs.com/masastack/${SsoServerName}:$t  -f $SsoDockerfilePath .
docker -H 192.168.51.153:2375 push registry.cn-hangzhou.aliyuncs.com/masastack/${SsoServerName}:$t 