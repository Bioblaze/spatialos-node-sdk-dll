var fs = require('fs');
var path = require('path');
var child_process = require('child_process');
var csproj = path.join(__dirname, "SpatialDLL", "SpatialDLL.csproj");

var msbuild = require(path.join(require.main.paths[0], 'msbuild'));

var StartCompile = new Promise(function(resolve, reject) {
  var build = new msbuild(function() {
    resolve();
  });
  build.sourcePath = csproj;
  build.package();
});


StartCompile.then(function() {
  console.log("Compile Completed!");
  return;
});
