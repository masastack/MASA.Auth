param($t,$u,$p,$s)

#docker build 
#Write-Host "Hello.$args"
Write-Host "Hello $t"

switch($s)
{
  service {$DockerfilePath="./src/Services/Masa.Auth.Service.Admin/Dockerfile";$ServerName="masa-auth-service-admin"}
  web  {$DockerfilePath="./src/Web/Masa.Auth.Web.Admin.Server/Dockerfile";$ServerName="masa-auth-web-admin-server"}
  sso  {$DockerfilePath="./src/Web/Masa.Auth.Web.Sso/Dockerfile";$ServerName="masa-auth-web-sso"}
}
docker login --username=$u registry.cn-hangzhou.aliyuncs.com --password=$p
docker build -t registry.cn-hangzhou.aliyuncs.com/masastack/${ServerName}:$t  -f $DockerfilePath .
docker push registry.cn-hangzhou.aliyuncs.com/masastack/${ServerName}:$t 