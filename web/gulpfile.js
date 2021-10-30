/// <binding AfterBuild='default' />
const gulp = require("gulp");
const concat = require('gulp-concat');
const gulpSass = require('gulp-sass');
const nodeSass = require('node-sass');
const sass = gulpSass(nodeSass);
const uglifyJs = require("uglify-js");
const through2 = require("through2");
const fsExtra = require("fs-extra");

const minifyJs = function (file, enc, callback) {
    var options = {
        annotations: false,
        mangle: {
            toplevel: true,
            eval: true
        },
        output: {
            beautify: false,
            braces: true,
            max_line_len: 100
        }
    };
    file.contents = Buffer.from(uglifyJs.minify(file.contents.toString(), options).code);
    callback(null, file);
}

gulp.task('clean:tmp', function () {
    fsExtra.emptyDirSync('wwwroot/css/');
    fsExtra.emptyDirSync('wwwroot/js/');
    return Promise.resolve('ok');
});


gulp.task('min:css', function () {
    return gulp.src(`css/*.scss`)
        .pipe(sass().on('error', sass.logError))
        .pipe(concat('styles.css'))
        .pipe(gulp.dest(`wwwroot/css/`));
});

gulp.task('min:js', function () {
    return gulp
        .src([`scripts/**/*.js`])
        //.pipe(through2.obj(minifyJs))
        .pipe(concat("bundle.js"))
        .pipe(gulp.dest(`wwwroot/js/`));
});

gulp.task("bundle", gulp.parallel(["min:css", "min:js"]));

gulp.task("default", gulp.series(["clean:tmp", "bundle"]));