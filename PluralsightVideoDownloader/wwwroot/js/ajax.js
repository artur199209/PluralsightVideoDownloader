$(document).ready(function () {
    $("#downloadCourseBtn").click(function () {
        $("#downloadCourseBtn").disabled = true;
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