var fs = require('fs');
var path = require('path');
var child_process = require('child_process');
const http = require('http');

var nuget_path = path.join(__dirname, "nuget.exe");

var StartInstall = new Promise(function(resolve, reject) {
  var file = fs.createWriteStream(nuget_path);
  http.get('http://www.nuget.org/nuget.exe', function(res) {
    res.pip(file);
  });
  file.on('finish', function() {
    console.log("nuget downloaded.");
    resolve();
  });
});

StartInstall.then(function() {
  var nuget = child_process.exec(nuget_path + " update -Self");
  nuget.stdout.pipe(process.stdout)
  nuget.on('exit', function() {
    console.log("nuget updating.")
    return;
  });
}).then(function() {
  var nuget = child_process.exec(nuget_path + " restore -SolutionDirectory ./");
  nuget.stdout.pipe(process.stdout)
  nuget.on('exit', function() {
    console.log('nuget Completed!');
    return;
  });
});
