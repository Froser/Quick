function submit(name, comment, modalname, modalbody, callback){
	if ($.trim(name) == "" || $.trim(comment) == ""){
		$('#' + modalbody).html("您的名字和评论不能为空哦。");
		$('#' + modalname).modal('show');
		callback();
		return false;
	} else {
		var submit_back = $.ajax({ url: 'api.aspx', data: { action: 'addcomment("' + name + '","' + comment + '")' },
	            success: function(){
		            $('#' + modalbody).html(submit_back.responseText);
		            $('#' + modalname).modal('show');
		            callback();
		        	},
	            async: true
	        });
		return true;
	}
}

function createCommentViewer(containerID, commentBoxID){
	$('#' + containerID).append(' \
		<div class="panel panel-info"> \
            <div class="panel-heading"> \
                <small id="comment_name_' + commentBoxID + '"><i class="icon-spinner icon-spin"></i></small> \
            </div> \
            <div class="panel-body small-font" id="comment_content_' + commentBoxID + '"> \
                <i class="icon-spinner icon-spin"></i> \
            </div> \
            <div class="panel-footer" id="comment_date_' + commentBoxID + '" style="text-align: right"><i class="icon-spinner icon-spin"></i></div> \
        </div> \
		');
}