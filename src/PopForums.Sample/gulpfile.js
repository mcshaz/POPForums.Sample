/// <binding BeforeBuild='merge-js, transfer' />

const gulp = require("gulp");
const merge = require("merge-stream");
const concat = require('gulp-concat');
const debug = require('gulp-debug');
// const minify = require('gulp-minify');
// const cleanCss = require('gulp-clean-css');

const nodeRoot = "./node_modules/";
const targetPath = "./wwwroot/lib/";

gulp.task("transfer", function () {
	var streams = [
		gulp.src(nodeRoot + "bootstrap/dist/**/*.min.css").pipe(gulp.dest(targetPath + "/bootstrap/dist")),
		gulp.src(nodeRoot + "@microsoft/signalr/dist/browser/**/*").pipe(gulp.dest(targetPath + "/signalr/dist")),
		gulp.src(nodeRoot + "tinymce/**/*").pipe(gulp.dest(targetPath + "/tinymce")),
		gulp.src(nodeRoot + "vue/dist/**/*").pipe(gulp.dest(targetPath + "/vue/dist")),
		gulp.src(nodeRoot + "vue-router/dist/**/*").pipe(gulp.dest(targetPath + "/vue-router/dist")),
		gulp.src(nodeRoot + "axios/dist/**/*").pipe(gulp.dest(targetPath + "/axios/dist")),
		gulp.src(nodeRoot + "@popworldmedia/popforums/**/*").pipe(gulp.dest(targetPath + "/PopForums"))];
	return merge(streams);
});

gulp.task("merge-js", function () {
	// the bundle package includes popper: "popper.js/dist/umd/popper.min.js"
	const jsstreams = ["jquery/dist/**/jquery.min.js", "bootstrap/dist/**/bootstrap.bundle.min.js"].map((s) => nodeRoot + s);
	return gulp.src(jsstreams)
		.pipe(debug())
		.pipe(concat('bundle.min.js'))
		.pipe(gulp.dest(targetPath));
});

