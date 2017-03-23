#!/bin/bash

if [[ -n "$TRAVIS_TAG" ]]
then
	if [[ `expr match $TRAVIS_TAG '^v[0-9]\+.[0-9]\+.[0-9]\+$'` -gt 0 ]]
	then
		APP_VERSION=${TRAVIS_TAG:1}.$TRAVIS_BUILD_NUMBER
		echo 'APP_VERSION:' $APP_VERSION

		echo '[assembly: AssemblyVersion("'$APP_VERSION'")]' >> src/Bitretsmah.UI.ConsoleApp/Properties/AssemblyInfo.cs
		echo '[assembly: AssemblyFileVersion("'$APP_VERSION'")]' >> src/Bitretsmah.UI.ConsoleApp/Properties/AssemblyInfo.cs
		echo '[assembly: AssemblyInformationalVersion("'$APP_VERSION'")]' >> src/Bitretsmah.UI.ConsoleApp/Properties/AssemblyInfo.cs
	else
		echo 'WRONG VERSION TAG FORMAT:' $TRAVIS_TAG
		echo 'VERSION TAG SHOULD BE LIKE: v1.2.3' 
		exit 1
	fi
fi

exit 0