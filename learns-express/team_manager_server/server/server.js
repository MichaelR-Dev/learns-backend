//!DEPENDENCIES
const dotenv = require('dotenv').config()
const { env } = require('process');

const express = require('express');
const cors = require('cors');

const whitelistOrigins = ["http://localhost:3000", "http://192.168.1.138:3000"]

var corsOptions = {
    origin: function (origin, callback) {
        if (whitelistOrigins.indexOf(origin) !== -1) {
        callback(null, true)
        } else {
        callback(new Error('Not allowed by CORS'))
        }
    },
    optionsSuccessStatus: 200 // some legacy browsers (IE11, various SmartTVs) choke on 204
}

const Connection = require('tedious').Connection
const Request = require('tedious').Request


let staffList = [];

const serverConfig = {
    server: 'localhost',
    authentication: {
      type: 'default',
      options: {
        userName: 'admin', // update me
        password: process.env.SQL_PASSWORD // update me
      }
    },
    options: {
        encrypt: true,
        trustServerCertificate: true
    }
}

const connection = new Connection(serverConfig)

connection.on('connect', (err) => {
    console.log("\nAttempting connection to SQLExpress...\n");
    if (err) {
        console.log(err)
    } else {
        console.log("Connected to SQL Server");
        executeStatement()
    }
})

function executeStatement () {

    request = new Request(`
    SELECT TOP (1000) [entry_id]
        ,[employee_id]
        ,[first_name]
        ,[last_name]
    FROM [team_manager].[dbo].[employees]`, (err, rowCount) => {
      if (err) {
        console.log(err)
      } else {
        //console.log(`${rowCount} rows`)
      }
      connection.close()
    })
  
    request.on('row', (columns) => {
        staffList.push(
            {
                name: `${columns[2].value + ' ' + columns[3].value}`,
                title: ""
            })
        console.log(`${columns[1].value} | ${columns[2].value} | ${columns[3].value}`)
    })
  
    connection.execSql(request)
}

connection.connect();

//const { errorHandler } = require('./src/middlewares/errorMiddleware');
//const { connectDB } = require('./src/cfg/db');
const path = require('path');
const { isArray } = require('util');

//!ENV VARIABLES
    //?SERVING PORT
const port = process.env.EXPRESS_PORT || 3000;

//*MODULE INITs
const app = express();

app.get('/', (req, res) => {
    res.send("Hello World");
})

//*MIDDLEWARE
const logger = (req, res, next) => {
    console.log(`${req.protocol}://${req.get('host')}${req.originalUrl}`);
    
    if(whitelistOrigins.find(str => {str === `${req.protocol}://${req.get('host')}${req.originalUrl}`})){
        res.header("Access-Control-Allow-Origin", `${req.protocol}://${req.get('host')}${req.originalUrl}`);
    }

    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
    next();
}

//---------------------------------------------------------------------
//connectDB();

app.use(logger);
//app.use(cors);
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

//app.use('/api/auth', require('./src/routes/authRoutes'));
//app.use('/api/members', require('./src/routes/memberRoutes'));

//app.use(errorHandler);

app.get('/api/staff', cors(corsOptions), (req, res) => {
    res.json(staffList);
})

app.listen(port, () =>{ console.log(`Server listening on port ${port}...`)});