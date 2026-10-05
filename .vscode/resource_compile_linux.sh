#!/bin/bash

printf "Compiling resources"

cd frontend/Resource
for file in `find ** -name "*.mgcb"` ;
do
    # Build the package
    dotnet tool run mgcb -r $file
    if [ $? != 0 ]; then
        exit 1
    fi
    echo "$file compiled"
done
