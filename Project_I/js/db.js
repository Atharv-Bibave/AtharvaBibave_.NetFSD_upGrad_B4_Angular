var db;

var request = indexedDB.open("EventDB", 1);

request.onupgradeneeded = function (e) {
  db = e.target.result;

  if (!db.objectStoreNames.contains("events")) {
    var store = db.createObjectStore("events", { keyPath: "id" });
    store.createIndex("name", "name", { unique: false });
    store.createIndex("category", "category", { unique: false });

    var defaultEvents = [
      {
        id: "101",
        name: "Dev Tech",
        category: "Tech & Innovations",
        date: "2026-03-04",
        time: "15:15",
        url: "https://www.upgrad.com",
      },
      {
        id: "102",
        name: "MCT Summit",
        category: "Tech & Innovations",
        date: "2026-03-09",
        time: "14:15",
        url: "https://www.upgrad.com",
      },
    ];

    for (var i = 0; i < defaultEvents.length; i++) {
      store.add(defaultEvents[i]);
    }
  }
};

request.onsuccess = function (e) {
  db = e.target.result;
  if (document.getElementById("eventContainer")) {
    loadAllEvents();
  }
  if (document.getElementById("homeEvents")) {
    loadHomeEvents();
  }
};

request.onerror = function () {
  alert("Database error. Please try again.");
};

function getAllEvents(callback) {
  var transaction = db.transaction(["events"], "readonly");
  var store = transaction.objectStore("events");
  var getAllRequest = store.getAll();

  getAllRequest.onsuccess = function () {
    callback(getAllRequest.result);
  };

  getAllRequest.onerror = function () {
    alert("Error reading events from database.");
  };
}

function addEventToDB(newEvent, callback) {
  var transaction = db.transaction(["events"], "readwrite");
  var store = transaction.objectStore("events");
  var addRequest = store.add(newEvent);

  addRequest.onsuccess = function () {
    callback(true);
  };

  addRequest.onerror = function () {
    alert("Event ID already exists. Please use a different ID.");
    callback(false);
  };
}

function deleteEventFromDB(eventId, callback) {
  var transaction = db.transaction(["events"], "readwrite");
  var store = transaction.objectStore("events");
  var deleteRequest = store.delete(eventId);

  deleteRequest.onsuccess = function () {
    callback();
  };

  deleteRequest.onerror = function () {
    alert("Error deleting event.");
  };
}
