const os = require('os');

console.log({
  cpus: os.cpus(),
  ram: os.uptime(),
  mem: {
    total: os.totalmem(),
    free: os.freemem()
  }
});