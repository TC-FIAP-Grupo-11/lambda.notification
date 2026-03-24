FROM public.ecr.aws/lambda/dotnet:8 AS base

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/FCG.Lambda.Notification/FCG.Lambda.Notification.csproj .
RUN dotnet restore
COPY src/FCG.Lambda.Notification/ .
RUN dotnet publish -c Release -r linux-x64 --no-self-contained -o /app/publish

FROM base AS final
COPY --from=build /app/publish ${LAMBDA_TASK_ROOT}
CMD ["FCG.Lambda.Notification::FCG.Lambda.Notification.Function::FunctionHandler"]
