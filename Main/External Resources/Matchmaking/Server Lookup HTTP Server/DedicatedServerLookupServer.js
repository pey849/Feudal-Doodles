#!/bin/env node
//  OpenShift sample Node application
var express = require('express');
var fs      = require('fs');


/*
    How to use:
        
        Create an openshift account and its nodejs application. Use this file 
        in place of server.js.

    How to test:
    
        curl feudaldoodleslookupserver-fightinggames.rhcloud.com:/api/addserver?ip=192.168.1.133
*/


var http = require('http');
var url = require('url');
var qrystr = require('querystring');

/* Constants */
var SERVER_DURATION_MIN = 30

/* Database of active servers */
var servers = [];


function printServers()
{
    console.log("Active Servers: %s", JSON.stringify(servers));
}

function deleteExpiredServers()
{
    var curDate = new Date();
    
    for (i = 0; i < servers.length; i++)
    {
        if (servers[i]['expiryDate'] <= curDate)
        {
            console.log("\n      Expired server:\n        %s", $JSON.stringify(servers[i]));
            servers.splice(i,1);
            i--;
        }
    }
}

function getClientIPv4(req)
{
    var ipv6 = req.connection.remoteAddress;
    
    var ipv4RegexMatches = ipv6.match(/^::ffff:(.+)$/);
    var ipv4 = ipv4RegexMatches[1];
    
    return ipv4
}

function generateRoomCode()
{
    /* Inclusive 1000 to 9999 */
    return Math.floor((Math.random() * 8999) + 1000).toString();
}

function addServer(ip)
{
    deleteExpiredServers();
    
    var l = null; 
    
    /* Check if server already exists */
    for (i = 0; i < servers.length; i++)
    {
        if (servers[i]['ip'] === ip)
        {
            l = i;
            break;
        }
    }
    
    var expiryDate = new Date();
    expiryDate.setMinutes( expiryDate.getMinutes() + SERVER_DURATION_MIN );
    
    var roomCode = null;
    
    /* If server doesn't exist */
    if (l === null)
    {
        var roomCode = generateRoomCode();
        var newServer = {'ip': ip, 
                        'roomCode': roomCode,
                        'expiryDate': expiryDate};
        servers.push(newServer);
    }
    else 
    {
        /* Update expiry time */
        servers[l]['expiryDate'] = expiryDate;
        
        roomCode = servers[l]['roomCode'];
    }
    
    printServers();
    
    return {'status': 'ok', 'roomCode': roomCode};
}

function removeServer(ip)
{
    deleteExpiredServers();
    
    for (i = 0; i < servers.length; i++)
    {
        if (servers[i]['ip'] == ip)
        {
            console.log("\n      Removed server:\n        %s", JSON.stringify(servers[i]));
            servers.splice(i,1);
            break;
        }
    }
    
    printServers();
    
    return false;
}

function queryServer(roomCode)
{
    deleteExpiredServers();
    
    var ip = null;
    
    for (i = 0; i < servers.length; i++)
    {
        if (servers[i]['roomCode'] == roomCode)
        {
            ip = servers[i]['ip'];
            break;
        }
    }
    
    if (ip == null)
        return {'status': 'notFound', 'ip': null}
    else
        return {'status': 'ok', 'ip': ip}
        
    printServers();
}









/**
 *  Define the sample application.
 */
var SampleApp = function() {

    //  Scope.
    var self = this;


    /*  ================================================================  */
    /*  Helper functions.                                                 */
    /*  ================================================================  */

    /**
     *  Set up server IP address and port # using env variables/defaults.
     */
    self.setupVariables = function() {
        //  Set the environment variables we need.
        self.ipaddress = process.env.OPENSHIFT_NODEJS_IP;
        self.port      = process.env.OPENSHIFT_NODEJS_PORT || 8080;

        if (typeof self.ipaddress === "undefined") {
            //  Log errors on OpenShift but continue w/ 127.0.0.1 - this
            //  allows us to run/test the app locally.
            console.warn('No OPENSHIFT_NODEJS_IP var, using 127.0.0.1');
            self.ipaddress = "127.0.0.1";
        };
    };


    /**
     *  Populate the cache.
     */
    self.populateCache = function() {
        if (typeof self.zcache === "undefined") {
            self.zcache = { 'index.html': '' };
        }

        //  Local cache for static content.
        self.zcache['index.html'] = fs.readFileSync('./index.html');
    };


    /**
     *  Retrieve entry (content) from cache.
     *  @param {string} key  Key identifying content to retrieve from cache.
     */
    self.cache_get = function(key) { return self.zcache[key]; };


    /**
     *  terminator === the termination handler
     *  Terminate server on receipt of the specified signal.
     *  @param {string} sig  Signal to terminate on.
     */
    self.terminator = function(sig){
        if (typeof sig === "string") {
           console.log('%s: Received %s - terminating sample app ...',
                       Date(Date.now()), sig);
           process.exit(1);
        }
        console.log('%s: Node server stopped.', Date(Date.now()) );
    };


    /**
     *  Setup termination handlers (for exit and a list of signals).
     */
    self.setupTerminationHandlers = function(){
        //  Process on exit and signals.
        process.on('exit', function() { self.terminator(); });

        // Removed 'SIGPIPE' from the list - bugz 852598.
        ['SIGHUP', 'SIGINT', 'SIGQUIT', 'SIGILL', 'SIGTRAP', 'SIGABRT',
         'SIGBUS', 'SIGFPE', 'SIGUSR1', 'SIGSEGV', 'SIGUSR2', 'SIGTERM'
        ].forEach(function(element, index, array) {
            process.on(element, function() { self.terminator(element); });
        });
    };


    /*  ================================================================  */
    /*  App server functions (main app logic here).                       */
    /*  ================================================================  */

    /**
     *  Create the routing table entries + handlers for the application.
     */
    self.createRoutes = function() {
        self.routes = { };

        self.routes['/asciimo'] = function(req, res) {
            var link = "http://i.imgur.com/kmbjB.png";
            res.send("<html><body><img src='" + link + "'></body></html>");
        };

        self.routes['/'] = function(req, res) {
            res.setHeader('Content-Type', 'text/html');
            res.send(self.cache_get('index.html') );
        };
        
        self.routes['/api/addserver'] = function(req, res) {
            var o_url = url.parse(req.url);
            var parsed_qrystr = qrystr.parse(o_url.query);
            
            var ip = parsed_qrystr['ip'];
            var rv = addServer(ip);
            
            res.setHeader('Content-Type', 'application/json');
            var resJson = JSON.stringify(rv);
            res.send(resJson)
        };
        
        self.routes['/api/removeserver'] = function(req, res) {
            var o_url = url.parse(req.url);
            var parsed_qrystr = qrystr.parse(o_url.query);
            
            var ip = parsed_qrystr['ip'];
            var rv = removeServer(ip);
            
            res.setHeader('Content-Type', 'application/json');
            var resJson = JSON.stringify({'status': 'ok'});
            res.send(resJson)
        };
        
        self.routes['/api/queryserver'] = function(req, res) {
            var o_url = url.parse(req.url);
            var parsed_qrystr = qrystr.parse(o_url.query);
            
            var rv = queryServer(parsed_qrystr['roomCode']);
            
            res.setHeader('Content-Type', 'application/json');
            var resJson = JSON.stringify(rv);
            res.send(resJson)
        };
    };


    /**
     *  Initialize the server (express) and create the routes and register
     *  the handlers.
     */
    self.initializeServer = function() {
        self.createRoutes();
        self.app = express.createServer();

        //  Add handlers for the app (from the routes).
        for (var r in self.routes) {
            self.app.get(r, self.routes[r]);
        }
    };


    /**
     *  Initializes the sample application.
     */
    self.initialize = function() {
        self.setupVariables();
        self.populateCache();
        self.setupTerminationHandlers();

        // Create the express server and routes.
        self.initializeServer();
    };


    /**
     *  Start the server (starts up the sample application).
     */
    self.start = function() {
        //  Start the app on the specific interface (and port).
        self.app.listen(self.port, self.ipaddress, function() 
        {
            console.log('%s: Node server started on %s:%d ...',
                        Date(Date.now() ), self.ipaddress, self.port); 
        });
    };

};   /*  Sample Application.  */



/**
 *  main():  Main code.
 */
var zapp = new SampleApp();
zapp.initialize();
zapp.start();

