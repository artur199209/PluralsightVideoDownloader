const connection = new signalR.HubConnectionBuilder()
    .withUrl("/myHub") 
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("notification", (message) => {
    $('#statusLogger').append(message + "\n");
    $('#statusLogger').scrollTop($('#statusLogger')[0].scrollHeight);
});

connection.on("UpdateProgressBar", (name, value) => {
    var id = name + "prb";
    var percentage = value + "%";
    var elem = document.getElementById(id);
    elem.style.width = percentage;
    elem.innerHTML = percentage; 
});

connection.on("InitProgressBar", (message) => {
    var div = document.getElementById('status');
    console.log(message);
    for (var i = 0; i < message.length; i++) {
        var htmlString =
            "<div class='form-group' id='>" + message[i] + "'" +
                "<label>" +
                message[i] +
                "</label>" +
                "<div class='progress'>" +
                "<div class='progress-bar' id='" +
                message[i] + "prb'" +
                "style='width: 0%'>0%</div>" +
                "</div></div>";
        div.innerHTML += htmlString;
    }
});

connection.on("Complete", () => {
    alert("Completed!!!");
    location.reload();
});

connection.on("Error", () => {
    alert("Error has occured. Please send me logs.");
    location.reload();
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});