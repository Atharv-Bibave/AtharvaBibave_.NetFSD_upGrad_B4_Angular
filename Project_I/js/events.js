function addEvent() {
  var id = document.getElementById("eventId").value.trim();
  var name = document.getElementById("eventName").value.trim();
  var category = document.getElementById("eventCategory").value;
  var date = document.getElementById("eventDate").value;
  var time = document.getElementById("eventTime").value;
  var url = document.getElementById("eventURL").value.trim();

  if (!id || !name || !category || !date || !time || !url) {
    alert("Please fill all fields");
    return;
  }

  var newEvent = {
    id: id,
    name: name,
    category: category,
    date: date,
    time: time,
    url: url,
  };

  addEventToDB(newEvent, function (success) {
    if (success) {
      document.getElementById("eventForm").reset();
      alert("Event added successfully!");
      loadAllEvents();
    }
  });
}

function loadAllEvents() {
  getAllEvents(function (eventList) {
    displayEvents(eventList);
  });
}

function displayEvents(eventList) {
  var container = document.getElementById("eventContainer");
  container.innerHTML = "";

  if (eventList.length === 0) {
    container.innerHTML = "<p class='text-center'>No events found</p>";
    return;
  }

  for (var i = 0; i < eventList.length; i++) {
    var ev = eventList[i];

    container.innerHTML +=
      '<div class="col-md-4 mb-3">' +
      '<div class="card p-3">' +
      "<h5>" +
      ev.name +
      "</h5>" +
      "<p><b>ID:</b> " +
      ev.id +
      "</p>" +
      "<p><b>Category:</b> " +
      ev.category +
      "</p>" +
      "<p><b>Date:</b> " +
      ev.date +
      "</p>" +
      "<p><b>Time:</b> " +
      ev.time +
      "</p>" +
      '<a href="' +
      ev.url +
      '" target="_blank" class="btn btn-primary w-100 mb-2">Join Event</a>' +
      '<button class="btn btn-danger w-100" onclick="deleteEvent(\'' +
      ev.id +
      "')\">Delete</button>" +
      "</div>" +
      "</div>";
  }
}

function deleteEvent(eventId) {
  if (confirm("Are you sure you want to delete this event?")) {
    deleteEventFromDB(eventId, function () {
      loadAllEvents();
    });
  }
}

function searchEvents() {
  var type = document.getElementById("searchType").value;
  var value = document.getElementById("searchValue").value.trim().toLowerCase();

  if (value === "") {
    alert("Please enter a search value");
    return;
  }

  getAllEvents(function (allEvents) {
    var filtered = [];

    for (var i = 0; i < allEvents.length; i++) {
      var ev = allEvents[i];
      if (type === "id" && ev.id.toString().trim() === value) {
        filtered.push(ev);
      } else if (type === "name" && ev.name.toLowerCase().includes(value)) {
        filtered.push(ev);
      } else if (
        type === "category" &&
        ev.category.toLowerCase().includes(value)
      ) {
        filtered.push(ev);
      }
    }

    displayEvents(filtered);
  });
}

function resetSearch() {
  document.getElementById("searchValue").value = "";
  loadAllEvents();
}
