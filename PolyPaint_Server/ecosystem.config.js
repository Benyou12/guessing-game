module.exports = {
    apps : [{
      name        : "Log3900 - API",
      script      : "sudo PORT=80 npm start",
      watch       : true,
      env: {
        "NODE_ENV": "development",
      },
      env_production : {
         "NODE_ENV": "production"
      }
    }]
  }