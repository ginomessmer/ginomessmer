const os = require('os');
const gh = require('@octokit/rest');

const secrets = require('./secrets.json');
let system = {};

const octokit = new gh.Octokit({
  auth: secrets.pat
});

console.log('Daemon started');

setInterval(() => uploadSystemStats().then(() => {
  console.log('Gist updated');
}), 5 * 1000);

const uploadSystemStats = () => {
  const cpus = os.cpus();

  system = {
    version: new Date(),
    cpu: {
      cores: cpus,
      avgCpuUsage: average(cpus.map(x => x.speed))
    },
    mem: {
      total: os.totalmem(),
      free: os.freemem()
    },
    uptime: os.uptime(),
  };

  return octokit.gists.update({
    gist_id: secrets.gistId,
    files: {
      'system-stats.json': {
        content: JSON.stringify(system, null, 2)
      }
    }
  });
}

const average = arr => arr.reduce((p, c) => p + c, 0) / arr.length;