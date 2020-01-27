#!/bin/bash

for i in "$@"
do
case $i in
    -p=*|--patchversion=*)
    BUILDNUMBER="${i#*=}"
    VERSION=$(<VERSION)
    FULLVERSION="${VERSION}.${BUILDNUMBER}"
    echo "Patching version: ${FULLVERSION}"
    sed -i -e "s/%VERSION%/${FULLVERSION}/g" src/Marvin.Runtime.Maintenance.Web.UI/src/Version.ts
    ;;
    *)
    # unknown option
    ;;
esac
done