import javax.swing.*;
import java.awt.event.*;
import java.awt.*;

import com.github.sarxos.webcam.Webcam;

public class WebcamGUI {
    private JTextField text;
    private Color color;
    public WebcamGUI(){
        Webcam webcam = Webcam.getDefault();
        System.out.println("Webcam: " + webcam.getName());

        JFrame frame = new JFrame("Webcam // cracker spotter");
        WebcamPanel webcamPanel = new WebcamPanel();
        new Thread(webcamPanel).start();
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        text = new JTextField("Webcam: " + webcam.getName());

        JButton button = new JButton("Press (:");

        button.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                if(ImageAnalyzer.isRed(webcamPanel.getImage())) text.setText("red");
                else if(ImageAnalyzer.isGreen(webcamPanel.getImage())) text.setText("green");
                else if(ImageAnalyzer.isBlue(webcamPanel.getImage())) text.setText("blue");
                else text.setText(ImageAnalyzer.getAverageColor(webcamPanel.getImage()).toString());
                color = ImageAnalyzer.getAverageColor(webcamPanel.getImage());
                button.setBackground(color);
            }
        });
        

        JPanel controlPanel = new JPanel();
        controlPanel.add(button);
        controlPanel.add(text);
        

        frame.add(controlPanel, BorderLayout.NORTH);
        frame.add(webcamPanel);
        frame.pack();
        frame.setVisible(true);
    }
}
