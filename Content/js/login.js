function togglePassword(){
    let pass = document.getElementById("pass");
    pass.type = pass.type === "password" ? "text" : "password";
}

function openForgot(){
    let modal = new bootstrap.Modal(document.getElementById('forgotModal'));
    modal.show();
}





// PAGE LOAD ANIMATION
window.addEventListener("load", ()=>{
    document.querySelector(".container-box").style.transform = "scale(1)";
    document.querySelector(".container-box").style.opacity = "1";
});

// INITIAL STATE
document.querySelector(".container-box").style.transform = "scale(0.95)";
document.querySelector(".container-box").style.opacity = "0";
document.querySelector(".container-box").style.transition = "all 0.6s ease";


// EXTRA SMOOTH SWITCH
function signUp(){
    let box = document.getElementById("container");
    box.classList.add("active");
    box.style.transform = "scale(1.02)";
    setTimeout(()=> box.style.transform="scale(1)", 200);
}

function signIn(){
    let box = document.getElementById("container");
    box.classList.remove("active");
    box.style.transform = "scale(1.02)";
    setTimeout(()=> box.style.transform="scale(1)", 200);
}













function togglePass(id){
    let input = document.getElementById(id);
    input.type = input.type === "password" ? "text" : "password";
}

function validateForm(){
    let pass = document.getElementById("pass").value;
    let cpass = document.getElementById("cpass").value;
    let msg = document.getElementById("msg");

    if(pass !== cpass){
        msg.innerText = "❌ Password does not match!";
        return false;
    }

    msg.innerText = "";
    alert("✅ Password Updated Successfully (UI only)");
    return true;
}















