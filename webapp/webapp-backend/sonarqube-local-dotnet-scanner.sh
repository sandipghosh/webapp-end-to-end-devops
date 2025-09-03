dotnet sonarscanner begin \
    /k:"webapp-local-dotnet-scanner" \
    /d:sonar.host.url="http://localhost:9000"  \
    /d:sonar.token="sqp_18fa54cb5d15541bd737bbb312a2f8beac858f7f" \
    /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml

# Need the following tool to get code coverage
# dotnet tool install --global dotnet-coverage
dotnet build ./webapp-backend.sln --no-incremental
dotnet-coverage collect "dotnet test" -f xml -o "coverage.xml"

dotnet sonarscanner end \
    /d:sonar.token="sqp_18fa54cb5d15541bd737bbb312a2f8beac858f7f"

