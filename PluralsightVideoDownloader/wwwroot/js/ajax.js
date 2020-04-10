$(document).ready(function () {
    $("#downloadCourseBtn").click(function () {
        $.ajax(
            {
                type: "POST",
                url: "Home/Test",  
                data: {
                    Name: $("#courseUrl").val()
                }
            });  
    });
});