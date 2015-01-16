var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var methodOverride = require('method-override');
var multer = require('multer')
var session = require('express-session');

var routes = require('./routes/index');
var users = require('./routes/users');
var calculator = require('./routes/calculator');

var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');

// uncomment after placing your favicon in /public
//app.use(favicon(__dirname + '/public/favicon.ico'));
app.use(logger('dev'));
//app.use(bodyParser.json());
//app.use(bodyParser.urlencoded({ extended: false }));
//app.use(express.methodOverride());
app.use(methodOverride());
app.use(multer());                                   // parse multipart/form-data
app.use(bodyParser({keepExtensions:true,uploadDir:path.join(__dirname,'/files')}));

app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

app.use(session({ secret: 'Web 101', cookie: { maxAge: 60000 }})) // unit is milliseconds one minute

app.use('/', routes);
app.use('/users', users);

app.get('/upload', calculator.form);
app.post('/upload', calculator.submit);
app.get('/calculate',calculator.calculate);

// TODO: why head doesn't work
app.head('/upload', function(req, res){
    console.log("begin head upload")
    res.send("ok")
});


// catch 404 and forward to error handler
app.use(function(req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function(err, req, res, next) {
        res.status(err.status || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use(function(err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});


module.exports = app;
