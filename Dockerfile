# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY SudokuServer/*.csproj ./SudokuServer/
COPY SudokuNet/DanceLinkX/*.csproj ./SudokuNet/DanceLinkX/
COPY SudokuNet/DanceLinkX.Default/*.csproj ./SudokuNet/DanceLinkX.Default/
COPY SudokuNet/Sudoku/*.csproj ./SudokuNet/Sudoku/
COPY SudokuNet/Sudoku.Default/*.csproj ./SudokuNet/Sudoku.Default/
RUN dotnet restore

# copy everything else and build app
COPY . ./
WORKDIR /source/SudokuServer
RUN dotnet publish -c release -o /app

# ef update
# 在这里update似乎有问题，我选择手动update
# RUN dotnet tool install --global dotnet-ef
# ENV PATH="$PATH:/root/.dotnet/tools"
# RUN dotnet ef database update

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "SudokuServer.dll"]
