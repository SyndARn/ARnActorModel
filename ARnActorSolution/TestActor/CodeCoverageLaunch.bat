
cd "..\packages\OpenCover.4.7.922\tools"
OpenCover.Console.exe "-register:user" -target:"C:\Program Files (x86)\Microsoft visual Studio\2019\Community\Common7\IDE\mstest.exe" -output:"..\..\..\TestActor\coverage.xml" -targetargs:"/testcontainer:..\..\..\TestActor\bin\Debug\TestActor.dll" -filter:+[*]* -mergebyhash
rem OpenCover.Console.exe "-register:user" -target:"C:\Program Files (x86)\Microsoft visual Studio \2019\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" -output:"..\..\TestActor\coverage.xml" -targetargs:"..\..\..\TestActor\bin\Debug\TestActor.dll" -filter:+[*]* -mergebyhash
pause
cd "..\..\..\packages\ReportGenerator.4.5.4\tools\net47"
"ReportGenerator.exe" "-reports:..\..\..\..\TestActor\coverage.xml" "-targetdir:..\..\..\..\TestActor\Report" -Reporttypes:Badges;Html;HtmlSummary
pause
