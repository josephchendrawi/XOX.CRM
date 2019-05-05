//load and refresh target dropdown from 1 dropdrown 

function LoadList() {
}

function LoadTargetDropDown(sourceddlID, targetddlID , postData) {
    var sourceID = $(sourceddlID).val();  //filter based on sourceID
    var targetDdl = $(targetddlID); 
    targetDdl.children('option:not(:first)').remove(); 

    $.post(postData, { sourceID: sourceID }, function (data) {
        var html = ""; 
        if (data != null) {
            data.forEach(function (Model) {
                html+= "<option value='"+Model.ItemValue+"'>"+Model.ItemText+"</option>"
            });
        }
    }).fail(function (data) {
        console.log(data);
    });
}
jQuery.fn.doubleHide = function () {
    return $(this).hide().css('opacity', '0');
}

jQuery.fn.slideFadeUp = function (slide, fade) {
    return $(this).slideUp(slide, function () {
        $(this).fadeTo(fade, 0);
    });
}
jQuery.fn.slideFadeDown = function (slide, fade) {
    return $(this).slideDown(slide, function () {
        $(this).fadeTo(fade, 100);
    });
}


$.fn.digits = function () {
    return this.each(function () {
        $(this).text($(this).text().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
    })
}