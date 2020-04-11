const connection = new signalR.HubConnectionBuilder()
    .withUrl("/myHub") 
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("notification", (message) => {
    $("#statusLog").innerHTML = "<li>" + message + "</li>";
});

connection.on("UpdateProgressBar", (name, value) => {
    var id = name + "prb";
    var percentage = value + "%";
    var elem = document.getElementById(id);
    elem.style.width = percentage;
    elem.innerHTML = percentage; 
});

connection.on("InitProgressBar", (message) => {
    var htmlString;
    var div = document.getElementById('status');
    
    for (var index = 0; index < message.length; index++) {
        htmlString = 
            "<div class='form-group' id='>" + message[index] + "'"+
            "<label>" +
            message[index] +
            "</label>" +
            "<div class='progress'>" +
            "<div class='progress-bar' id='" +
            message[index] + "prb'"+
            "style='width: 0%'>0%</div>" +
            "</div></div>";
        div.innerHTML += htmlString;
    }
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});