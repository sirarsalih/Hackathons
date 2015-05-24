# If Hackathon 2015
Tweet it, read it!

<h2>Team</h2>
Sirar Salih

<h2>Setup</h2>

1. Install <a href="http://git-scm.com/downloads" target="_blank">git</a>.
2. Install <a href="http://nodejs.org/download/" target="_blank">Node.js</a>.
3. Install <a href="https://code.visualstudio.com/" target="_blank">Visual Studio Code</a> or any other editor of your choice.
4. <strike>Go to <a href="https://twitter.com/" target="_blank">Twitter</a> and create an If Hackathon 2015 account (one time only).</strike> <a href="https://twitter.com/IfHackathon2015" target="_blank">@IfHackathon2015</a>.
5. <strike>Go to <a href="https://apps.twitter.com/" target="_blank">Twitter Apps</a> and create an app (one time only).</strike>
6. <strike>Fetch the <code>consumer key</code>, <code>consumer secret</code>, <code>token key</code> and <code>token secret</code> from the app created in step 5. We will use these for authentication (one time only).</strike>
7. Run <code>git clone https://github.com/sirarsalih/if-hackathon-2015.git</code> to fetch this repo.
8. Navigate to <code>package.json</code> and run <code>npm install -g</code> to install dependencies.
9. Write your awesome code in <code>advanced_twitter_bot.js</code>.

<h2>Run the App</h2>

To run the app locally simply execute:

<code>nodemon advanced_twitter_bot.js</code>

<h2>Deploy to RaspberryPi</h2>

The app will be manually deployed to a <a href="https://raspi.sirars.com/" target="_blank">RaspberryPi</a> for testing. We will push the code to https://github.com/sirarsalih/raspberrypi/tree/master/home/pi/twitter/if_hackathon_2015. A daily cron job will then run the app.
