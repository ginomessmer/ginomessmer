const os = require('os');
const sys = require('systeminformation');
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

const uploadSystemStats = async () => {
  const cpu = await sys.cpu();
  const cpuSpeed = await sys.cpuCurrentspeed();

  const memory = await sys.mem();

  system = {
    version: new Date(),
    cpu: {
      details: cpu,
      speed: cpuSpeed
    },
    memory: memory,
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