$(document).ready( function () {
    var page = window.location.pathname;
    var htmlajax = $.ajax({ url: 'visit.aspx', data: { page: page }, async: true });
    $('head').append('<METAHTTP-EQUIV="Pragma"CONTENT="no-cache">');
    $('head').append('<METAHTTP-EQUIV="Cache-Control"CONTENT="no-cache">');
    $('head').append('<METAHTTP-EQUIV="Expires"CONTENT="0">');
    $('head').append('<link rel="shortcut icon" href="favicon.ico">');
    $('head').append('<link href="css/framework.css" rel="stylesheet" />');
    $('head').append('<link rel="stylesheet" href="js/font-Awesome-3.2.1/css/font-awesome.min.css">');
	$('body').prepend ('\
    <div class="container">\
    	<nav class="navbar navbar-inverse navbar-collapse navbar-fixed-top" role="navigation">\
            <div class="navbar-header">\
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#bs-navbar-collapse">\
                    <span class="sr-only">Toggle navigation</span>\
                    <span class="icon-bar"></span>\
                    <span class="icon-bar"></span>\
                    <span class="icon-bar"></span>\
                    <span class="icon-bar"></span>\
                </button>\
                <a class="navbar-brand" href="index.html"> <img alt="Quick" src="img/quick.png" /> Quick</a>\
            </div>\
            <div class="collapse navbar-collapse" id="bs-navbar-collapse">\
                <ul class="nav navbar-nav">\
                    <li id="nav-li-index"><a href="index.html">主页</a></li>\
                    <li id="nav-li-plugins"><a href="plugins.html">插件平台</a></li>\
                    <li id="nav-li-plugindev"><a href="plugindev.html">插件开发</a></li>\
                    <li id="nav-li-comment"><a href="comment.html">留言评论</a></li>\
                </ul>\
                <p class="navbar-text navbar-right">Froser Presents</p>\
            </div>\
        </nav>\
    </div>\
		');
	var active_nav_li = $('#nav-li-' + $('html').attr('navname'));
	$(active_nav_li).attr("class","active");

});