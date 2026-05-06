function login(e) {
  e.preventDefault();

  var email = document.getElementById("email").value.trim();
  var password = document.getElementById("password").value.trim();

  if (email === "admin@upgrad.com" && password === "12345") {
    sessionStorage.setItem("isLoggedIn", "true");
    window.location.href = "events.html";
  } else {
    alert("Invalid login credentials");
  }
}

function checkAuth() {
  if (sessionStorage.getItem("isLoggedIn") !== "true") {
    alert("Unauthorized access. Please login first.");
    window.location.href = "login.html";
  } else {
    document.body.style.visibility = "visible";
  }
}

function logout() {
  sessionStorage.removeItem("isLoggedIn");
  window.location.href = "login.html";
}
