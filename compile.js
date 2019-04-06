var fs = require('fs');
var path = require('path');
var child_process = require('child_process');
var csproj = path.join(__dirname, "SpatialDLL", "SpatialDLL.csproj");

var StartCompile = new Promise(function(resolve, reject) {
  var msbuild = child_process.exec(`msbuild ${csproj} /p:Configuration=Release`);
  msbuild.stdout.pipe(process.stdout);
  msbuild.on('exit', function() {
    resolve();
  });
});


StartCompile.then(function() {
  console.log("Compile Completed!");
  return;
});
