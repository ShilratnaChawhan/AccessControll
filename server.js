const express = require("express");
const path = require("path");

const app = express();

// Serve static files from the Angular build folder
app.use(express.static(path.join(__dirname, "app")));

// Redirect all routes to index.html (for Angular routing)
app.get("*", (req, res) => {
  res.sendFile(path.join(__dirname, "app", "index.html"));
});

// Set the port (default: 3000)
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`✅ Angular app is running on http://localhost:${PORT}`);
});
