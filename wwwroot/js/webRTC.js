const configuration = { 
// "iceServers": [{ "url": "stun:stun.example.org" }]
};

const offerOptions = {
  offerToReceiveAudio: 1,
  offerToReceiveVideo: 1
};

class WebRTC{
  localStream;
  pc;
  socket;
  constructor(socket, localStream){
    this.socket = socket;
    this.localStream = localStream;
    this.pc = new RTCPeerConnection(configuration);
    console.log(this.pc)
    // 當有任何 ICE candidates 可用時，
    // 透過 signalingChannel 將 candidate 傳送給對方
    this.pc.onicecandidate = function (evt) {
      console.log('onicecandidate:', evt.candidate);

      if (evt.candidate)
        socket.send(JSON.stringify({ "candidate": evt.candidate }));
    };

    this.pc.onaddstream = e => { 
      console.log('onaddstream')
    };


    // 監聽 ICE 連接狀態
    this.pc.oniceconnectionstatechange = (evt) => {
      console.log('ICE 伺服器狀態變更 => ', evt.target.iceConnectionState);
    };

    this.pc.onicegatheringstatechange = (evt) => {
      console.log('ICE 伺服器狀態變更 => ', evt.target.iceConnectionState);
    };
  }

  async createOffer(){
    console.log('createOffer')
    var offer = await this.pc.createOffer(offerOptions);
    await this.localDescCreated(offer)
  }

  async createAnswer(){
    console.log('createAnswer')
    var offer = await this.pc.createAnswer(offerOptions);
    await this.localDescCreated(offer)
  }

  addStream(stream){
    console.log('addstream')
    this.pc.addStream(stream)
  }

  addIceCandidate(candidate){
    console.log('addIceCandidate')
    this.pc.addIceCandidate(candidate)
  }

  async localDescCreated(desc) {
    await this.pc.setLocalDescription(desc);
    console.log('setLocalDescription:', desc)
    this.socket.send(JSON.stringify({ "sdp": this.pc.localDescription }));
  }

  close(){
    this.pc.close()
    this.pc = null
  }

  logError(error) {
    console.error(error.name + ": " + error.message);
  }
}
