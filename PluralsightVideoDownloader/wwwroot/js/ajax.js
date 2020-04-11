$(document).ready(function () {
    $("#downloadCourseBtn").click(function () {
        $('#downloadCourseBtn').attr('disabled', 'disabled');
        $.ajax(
            {
                type: "POST",
                url: "Home/Index2",
                data: {
                    url: $("#courseUrl").val()
                }
            });  
    });
});