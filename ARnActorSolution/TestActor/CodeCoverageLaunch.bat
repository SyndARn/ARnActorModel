
cd "C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\packages\OpenCover.4.7.922\tools"
echo "opencover.console.exe" "-register:user" -target:"C:\Program Files (x86)\Microsoft visual Studio 14.0\Common7\IDE\mstest.exe" -output:"C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\TestActor\coverage.xml" -targetargs:"/testcontainer:C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\TestActor\bin\Debug\TestActor.dll" -filter:+[*]* -mergebyhash
"opencover.console.exe" "-register:user" -target:"C:\Program Files (x86)\Microsoft visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" -output:"C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\TestActor\coverage.xml" -targetargs:"C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\TestActor\bin\Debug\TestActor.dll" -filter:+[*]* -mergebyhash
cd "C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\TestActor\"
pause
cd "C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\packages\ReportGenerator.4.5.4\tools\net47"
"ReportGenerator.exe" "-reports:C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\TestActor\coverage.xml" "-targetdir:C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\TestActor\Report" -Reporttypes:Badges;Html;HtmlSummary
cd "C:\Users\ARn\Source\Repos\ARnActorModel\ARnActorSolution\TestActor\"
pause
