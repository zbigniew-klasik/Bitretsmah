#!/bin/bash

cp src/Bitretsmah.UI.ConsoleApp/bin/Release/Bitretsmah.UI.ConsoleApp.exe ./app.exe
cp src/Bitretsmah.UI.ConsoleApp/bin/Release/*.dll ./

DLLS_NAMES=`ls *.dll`

mono src/packages/ILRepack.2.0.12/tools/ILRepack.exe /out:bitretsmah.exe app.exe $DLLS_NAMES

exit 0