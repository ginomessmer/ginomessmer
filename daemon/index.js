const os = require('os');
const gh = require('@octokit/rest');

const secrets = require('./secrets.json');
let system = {};

const octokit = new gh.Octokit({
  auth: secrets.pat
});

console.log('Daemon started');

setInterval(() => {
  uploadSystemStats().then(x => {
    console.log('Gist updated');
  });

}, 5 * 1000);

const uploadSystemStats = () => {
  system = {
    version: new Date(),
    cpus: os.cpus(),
    uptime: os.uptime(),
    mem: {
      total: os.totalmem(),
      free: os.freemem()
    }
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