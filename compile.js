var fs = require('fs');
var path = require('path');
var spawn = require('child_process').spawn;

//Taken from Edge-js's whereis.js ~ THANK GOD FOR THEM!
var getFromPath = function() {
  var pathSep = process.platform === 'win32' ? ';' : ':';
  var directories = process.env.PATH.split(pathSep);
  for (var i = 0; i < directories.length; i++) {
    for (var j = 0; j < arguments.length; j++) {
      var filename = arguments[j];
        var filePath = path.join(directories[i], filename);

        if (fs.existsSync(filePath)) {
            return filePath;
        }
    }
  }
  return null;
}


var csproj = path.join(__dirname, "SpatialDLL", "SpatialDLL.csproj");
var StartCompile = new Promise(function(resolve, reject) {
  var dotnet = getFromPath('dotnet', 'dotnet.exe');
  spawn(dotnet, ['build', csproj, '--configuration', 'Release'], { stdio: 'inherit', cwd: path.resolve(__dirname) })
  .on('close', function() {
    resolve();
  });
});

StartCompile.then(function() {
  console.log("Compile Completed!");
  return;
});
