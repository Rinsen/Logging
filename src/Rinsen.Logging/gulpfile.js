/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

/// <binding Clean='clean' />
"use strict";

var gulp = require('gulp'),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var webRoot = "./wwwroot/";
var webRootPaths = {
    js: webRoot + "js/**/*.js",
    minJs: webRoot + "js/**/*.min.js",
    css: webRoot + "css/**/*.css",
    minCss: webRoot + "css/**/*.min.css",

    concatJsDest: webRoot + "js/site.min.js",
    concatCssDest: webRoot + "css/site.min.css"
};

var npmPaths = {
    bootstrap: "./node_modules/bootstrap/dist/css/**.css"
};

var destPaths = {
    css: webRoot + "/css/"
};

gulp.task("clean:js", function (cb) {
    rimraf(webRootPaths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(webRootPaths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([webRootPaths.js, "!" + webRootPaths.minJs], { base: "." })
        .pipe(concat(webRootPaths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([webRootPaths.css, "!" + webRootPaths.minCss])
        .pipe(concat(webRootPaths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("bootstrap:css", function () {
    return gulp.src([npmPaths.bootstrap])
        //.pipe(concat(webRootPaths.concatCssDest))
        //.pipe(cssmin())
        .pipe(gulp.dest(destPaths.css));
});

gulp.task("min", ["min:js", "min:css"]);