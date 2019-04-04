var fs = require('fs');
var path = require('path');
var child_process = require('child_process');
var request = require('request');
var async = require('async');

var nuget_path = path.join(__dirname, "..", "nuget.exe");

async.waterfall([
  function(cb) { // Downloads nuget.exe
    var file = fs.createWriteStream(nuget_path);
    request.get('http://www.nuget.org/nuget.exe').pipe(file);
    file.on('finish', function() {
      cb(null);
    });
  },
  function(cb) { // Updates nuget.exe
    var nuget = child_process.exec(nuget_path + " update -Self");
    nuget.stdout.pipe(process.stdout)
    nuget.on('exit', function() {
      cb(null);
    });
    //child_process.spawn(nuget_path, ["update", "-Self"]);

  },
  function(cb) { // Restores Packages with nuget.exe
    var nuget = child_process.exec(nuget_path + " restore -SolutionDirectory ./");
    nuget.stdout.pipe(process.stdout)
    nuget.on('exit', function() {
      cb(null);
    });
  }
], function(err, res) {
  console.log('nuget Completed!');
  process.exit(1);
});
