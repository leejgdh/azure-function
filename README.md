# 기본 Azure function 예제

dotnet 8 에서 사용하기 위한 azure function 기본 기능들을 모아두기 위해서 업로드

dotnet 8에 azure function v4 기준으로 계속 만들어 나갈 예정

config 값 사용을 위한 local.settings.json은 별도로 작성해둠



## local.settings.json

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  },
  "ConnectionStrings": {
    "MyDatabase": "{your connection string}"
  }
}

```

# OpenApi Extension 추가
[azure-functions-openapi-extension](https://github.com/Azure/azure-functions-openapi-extension/blob/main/docs/enable-open-api-endpoints-out-of-proc.md)

Azure function에서 사용가능한 Open API Extension 추가.
Swagger ui 지원
package는 dotnet7 까지만 지원 한다고 되어 있으나, 업데이트 안한지 2년이 지나긴함.

OpenApiSecurity를 추가하여 Identity를 이용한 API 보호 처리하였음.
다만 인증 부분을 자동으로 확인하는건 아직 못찾아서, 코드로 따로 처리해줘야 하는데, Middleware등을 이용한 방법을 찾거나, 다른 방법을 좀 찾아봐야 할듯
