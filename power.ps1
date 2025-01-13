param($t,$u,$p)

#docker build 
Write-Host "Hello $t"

docker login --username=$u registry.cn-hangzhou.aliyuncs.com --password=$p

$ServiceDockerfilePath="./src/Services/Masa.Auth.Service.Admin/Dockerfile"
$ServiceServerName="masa-auth-service-admin"
$WebDockerfilePath="./src/Web/Masa.Auth.Web.Admin.WebAssembly/Dockerfile"
$WebServerName="masa-auth-web-admin"
$SsoDockerfilePath="./src/Web/Masa.Auth.Web.Sso/Dockerfile"
$SsoServerName="masa-auth-web-sso"

docker build -t registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t  -f $ServiceDockerfilePath .
docker push registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t 

docker build -t registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t  -f $WebDockerfilePath .
docker push registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t 

docker build -t registry.cn-hangzhou.aliyuncs.com/masastack/${SsoServerName}:$t  -f $SsoDockerfilePath .
docker push registry.cn-hangzhou.aliyuncs.com/masastack/${SsoServerName}:$t 