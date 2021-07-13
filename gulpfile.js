/// <binding AfterBuild='compile:sass, bundle:scripts' Clean='default:clean' ProjectOpened='watch:sass' />
let gulp = require('gulp');
let sass = require('gulp-sass')(require('sass'));
let rimraf = require('rimraf');

var concat = require('gulp-concat');
var rename = require('gulp-rename');
var uglify = require('gulp-uglify');

gulp.task('compile:sass', () => {
    return gulp.src(`./wwwroot/scss/**/*.scss`)
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest(`./wwwroot/css/`));
});

gulp.task('watch:sass', () => {
    gulp.watch('./wwwroot/scss/**/*.scss', (done) => {
        gulp.series(['compile:sass'])(done);
    });
});

gulp.task('clean:css', (callback) => {
    rimraf('./wwwroot/css/', callback);
});

const jsRoot = './wwwroot/js';
gulp.task('bundle:scripts', () => {
    return gulp.src('$(jsRoot)/**/*.js')
        .pipe(concat('bundle.js'))
        .pipe(gulp.dest(jsRoot))
        .pipe(rename('bundle.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest(jsRoot));
});

gulp.task('clean:scripts', (callback) => {
    rimraf(`./wwwroot/js/bundle.js`, callback);
    rimraf(`./wwwroot/js/bundle.min.js`, callback);
});

gulp.task('default:clean', gulp.series(
    'clean:css',
    'clean:scripts'
));