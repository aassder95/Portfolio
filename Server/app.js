const express = require("express");
const bodyParser = require("body-parser");
const dotenv = require("dotenv");
const protocolRoutes = require("./routes/protocolRoutes");
const connectDatabase = require("./utils/connectDatabase");
require("./models/index.js");

dotenv.config();

const app = express();

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

connectDatabase();

app.use("/", protocolRoutes);

module.exports = app;
