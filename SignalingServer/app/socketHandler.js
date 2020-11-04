module.exports = function(io, streams) {

  io.on('connection', function(client) {
    console.log('-- ' + client.id + ' joined --');
	let clientId = { id: client.id };
    client.emit('id', clientId);
	//client.emit('id', client.id);

    client.on('message', function (details) {
      console.log('on message ' + JSON.stringify(details));
      var otherClient = io.sockets.connected[details.to];

      if (!otherClient) {
		console.log('no such client found');
        return;
      }
        delete details.to;
        details.from = client.id;
        otherClient.emit('message', details);
    });
      
    client.on('readyToStream', function(options) {
      console.log('-- ' + client.id + ' is ready to stream --');
      
      streams.addStream(client.id, options.name); 
    });
    
    client.on('update', function(options) {
      streams.update(client.id, options.name);
    });

    function leave() {
      console.log('-- ' + client.id + ' left --');
      streams.removeStream(client.id);
    }

    client.on('disconnect', leave);
    client.on('leave', leave);
  });
};