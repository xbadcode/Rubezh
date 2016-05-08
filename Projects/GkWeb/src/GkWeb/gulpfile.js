/// <binding Clean='clean:libs' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf")
var bower = require('gulp-bower');

var paths = {
	webroot: "./wwwroot/",
	libs: [
        'node_modules/es6-shim/es6-shim.min.js',
        'node_modules/es6-shim/es6-shim.map',
        'node_modules/zone.js/dist/zone.js*',
        'node_modules/reflect-metadata/Reflect.js*',
        'node_modules/systemjs/dist/system.src.js*'
	],
	packages: [
        '@angular/common',
        '@angular/compiler',
        '@angular/core',
        '@angular/http',
        '@angular/platform-browser',
        '@angular/platform-browser-dynamic',
        '@angular/router'
	]
};


paths.concatJsDest = paths.webroot + "lib";

gulp.task("clean:libs", function (cb) {
	rimraf(paths.concatJsDest, cb);
});

gulp.task('bower', function() {
  return bower().pipe(gulp.dest('./wwwroot/lib/'));;
});

gulp.task('copyLibs:angular', function () {
	gulp.src(paths.libs).pipe(gulp.dest('./wwwroot/lib/etc'));
	gulp.src('node_modules/rxjs/**/*.js*').pipe(gulp.dest('./wwwroot/lib/rxjs'));

	for (var i = 0; i < paths.packages.length; i++) {
		gulp.src('node_modules/' + paths.packages[i] + '/*.js*').pipe(gulp.dest('./wwwroot/lib/' + paths.packages[i]));
		gulp.src('node_modules/' + paths.packages[i] + '/src/**/*.js*').pipe(gulp.dest('./wwwroot/lib/' + paths.packages[i] + '/src/'));
	}
});

gulp.task("copyLibs:d3", function () {
	gulp.src(['./node_modules/d3/d3*.js'])
	.pipe(gulp.dest('./wwwroot/lib/d3/'));
});

gulp.task("copyLibs:winjs", function () {
	gulp.src(['./node_modules/winjs/js/*.js', './node_modules/winjs/css/*.css', './node_modules/winjs/fonts/*.ttf'])
	.pipe(gulp.dest('./wwwroot/lib/winjs/'));
});

gulp.task("copyLibs", ["copyLibs:angular", "copyLibs:d3", "copyLibs:winjs"]);