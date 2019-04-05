var fs = require('fs');
var path = require('path');
var child_process = require('child_process');
const http = require('https');

var nuget_path = path.join(__dirname, "nuget.exe");

var StartInstall = new Promise(function(resolve, reject) {
  var file = fs.createWriteStream(nuget_path);
  http.get('https://www.nuget.org/nuget.exe', function(response) { // This redirects.. LOL :P
      http.get(response.headers.location, function(res) {
        res.pipe(file);
        file.on('finish', function() {
          file.close();
          console.log("nuget downloaded.");
          resolve();
        });
      });
  });
});

StartInstall.then(function() {
  var nuget = child_process.exec("nuget.exe update -Self");
  nuget.stdout.pipe(process.stdout)
  nuget.on('exit', function() {
    console.log("nuget updated.");
    var nuget = child_process.exec("nuget.exe restore -SolutionDirectory ./");
    nuget.stdout.pipe(process.stdout)
    nuget.on('exit', function() {
      console.log('nuget Completed!');
      return;
    });
  });
})
