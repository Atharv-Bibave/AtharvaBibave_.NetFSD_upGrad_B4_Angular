function goToRegister(id, name, category, date, time) {
  var event = {
    id: id,
    name: name,
    category: category,
    date: date,
    time: time,
  };

  localStorage.setItem("selectedEvent", JSON.stringify(event));
  window.location.href = "register.html";
}

function loadEventDetails() {
  var event = JSON.parse(localStorage.getItem("selectedEvent"));

  if (!event) {
    alert("No event selected. Redirecting to home page.");
    window.location.href = "index.html";
    return;
  }

  document.getElementById("eventId").innerText = event.id;
  document.getElementById("eventName").innerText = event.name;
  document.getElementById("eventCategory").innerText = event.category;
  document.getElementById("eventDate").innerText = event.date;
  document.getElementById("eventTime").innerText = event.time;
}

function registerParticipant(e) {
  e.preventDefault();
  alert("You are successfully registered to this event!");
  document.getElementById("registrationForm").reset();
}
